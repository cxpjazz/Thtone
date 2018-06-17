using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreHelper.Encrypt;

namespace CoreHelper.Encrypt
{
    public class Rsa
    {
        #region 密钥对，以下三组格式的密钥对可以相互转换,可以使用StringHelper中的转换类
        /// <summary>
        /// 密钥对，以下三组格式的密钥对可以相互转换,可以使用StringHelper中的转换类
        /// </summary>
        public class MapKeys
        {
            /// <summary>
            /// 16进制公钥509格式,常用于java
            /// </summary>
            public string HexPublicKey { get; set; }
            /// <summary>
            /// 16进制私钥509格式,常用于java
            /// </summary>
            public string HexPrivateKey { get; set; }

            /// <summary>
            /// Xml格式公钥，用于.net
            /// </summary>
            public string XmlPublicKey { get; set; }
            /// <summary>
            /// Xml格式私钥,用于.net
            /// </summary>
            public string XmlPrivateKey { get; set; }

            /// <summary>
            /// Base64格式公钥,用于.net
            /// </summary>
            public string Base64PublicKey { get; set; }
            /// <summary>
            /// Base64格式私钥,用于.net
            /// </summary>
            public string Base64PrivateKey { get; set; }

        } 
        #endregion

        #region 生成密钥对，正式使用时只需要调用一次，生成的密钥对需要保存，每次生成都不一样
        /// <summary>
        /// 生成密钥对，正式使用时只需要调用一次，生成的密钥对需要保存，每次生成都不一样
        /// </summary>
        public static MapKeys CreateKey()
        {
            MapKeys mapkeys = new MapKeys();
            //生成密钥对  
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            RsaKeyGenerationParameters rsaKeyGenerationParameters = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new Org.BouncyCastle.Security.SecureRandom(), 1024, 25);
            rsaKeyPairGenerator.Init(rsaKeyGenerationParameters);//初始化参数  
            AsymmetricCipherKeyPair keyPair = rsaKeyPairGenerator.GenerateKeyPair();
            AsymmetricKeyParameter publicKey = keyPair.Public;//公钥  
            AsymmetricKeyParameter privateKey = keyPair.Private;//私钥  

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

            //生成byte密钥数据
            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded();
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded();

            //16进制密钥对
            string HexPublicKey = StringHelper.ByteToHex(publicInfoByte);
            string HexPrivateKey = StringHelper.ByteToHex(privateInfoByte);
            mapkeys.HexPublicKey = HexPublicKey;
            mapkeys.HexPrivateKey = HexPrivateKey;

            //Base64密钥对
            string Base64PublicKey = StringHelper.HexToBase64String(HexPublicKey);
            string Base64PrivateKey = StringHelper.HexToBase64String(HexPrivateKey);
            mapkeys.Base64PublicKey = Base64PublicKey;
            mapkeys.Base64PrivateKey = Base64PrivateKey;

            //Xml密钥对
            mapkeys.XmlPublicKey = RSAPublicKeyJava2DotNet(Base64PublicKey);
            mapkeys.XmlPrivateKey = RSAPrivateKeyJava2DotNet(Base64PrivateKey);
            
            return mapkeys;
        }
        #endregion


        #region RSA私钥格式转换，java->.net,hex->xml
        /// <summary>
        /// RSA私钥格式转换，java->.net,hex->xml
        /// </summary>
        /// <param name="privateKey">java生成的RSA私钥</param>
        /// <returns></returns>
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }
        #endregion

        #region RSA私钥格式转换，.net->java
        /// <summary>
        /// RSA私钥格式转换，.net->java
        /// </summary>
        /// <param name="privateKey">.net生成的私钥</param>
        /// <returns></returns>
        public static string RSAPrivateKeyDotNet2Java(string privateKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(privateKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
            BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
            BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
            BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
            BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));

            RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);

            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }
        #endregion

        #region RSA公钥格式转换，java->.net,hex->xml
        /// <summary>
        /// RSA公钥格式转换，java->.net,hex->xml
        /// </summary>
        /// <param name="publicKey">java生成的公钥</param>
        /// <returns></returns>
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }
        #endregion

        #region RSA公钥格式转换，.net->java
        /// <summary>
        /// RSA公钥格式转换，.net->java
        /// </summary>
        /// <param name="publicKey">.net生成的公钥</param>
        /// <returns></returns>
        public static string RSAPublicKeyDotNet2Java(string publicKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            RsaKeyParameters pub = new RsaKeyParameters(false, m, p);

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }
        #endregion


        #region RSA签名,验证
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data">待签名的内容</param>
        /// <param name="privateKeyJava">16进制私钥</param>
        /// <param name="signature">签名</param>
        /// <param name="hashAlgorithm">模式，默认SHA1</param>
        /// <param name="encoding">编码，默认utf-8</param>
        /// <returns></returns>
        public static string RSASign(string data, string privateKeyJava, string hashAlgorithm = "SHA1", string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromPrivateKeyJavaString(privateKeyJava);//加载私钥
            var dataBytes = Encoding.GetEncoding(encoding).GetBytes(data);
            var HashbyteSignature = rsa.SignData(dataBytes, hashAlgorithm);
            return StringHelper.ByteToHex(HashbyteSignature);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <param name="publicKeyJava">16进制公钥</param>
        /// <param name="signature">签名</param>
        /// <param name="hashAlgorithm">模式，默认SHA1</param>
        /// <param name="encoding">编码，默认utf-8</param>
        /// <returns></returns>
        public static bool Verify(string data, string publicKeyJava, string signature, string hashAlgorithm = "SHA1", string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //导入公钥，准备验证签名
            rsa.FromPublicKeyJavaString(publicKeyJava);
            //返回数据验证结果
            byte[] Data = Encoding.GetEncoding(encoding).GetBytes(data);
            byte[] rgbSignature = Convert.FromBase64String(signature);

            return rsa.VerifyData(Data, hashAlgorithm, rgbSignature);
        }
        #endregion

        #region RSA加密,解密
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="sourcedata">待加密字符串</param>
        /// <param name="HexPublickey">16进制公钥</param>
        /// <returns>16进制字符串</returns>
        public string Encrypt(string sourcedata, string HexPublickey)
        {
            //明文转byte[]
            byte[] messagebytes = Encoding.UTF8.GetBytes(sourcedata);

            //公钥,Hex转Base64
            string publickey = RSAPublicKeyJava2DotNet(HexPublickey);

            //公钥加密 
            RSACryptoServiceProvider oRSA1 = new RSACryptoServiceProvider();
            oRSA1.FromXmlString(publickey); //加密要用到公钥，导入公钥 
            byte[] AOutput = oRSA1.Encrypt(messagebytes, false); //AOutput加密以后的数据 
            return StringHelper.ByteToHex(AOutput);//byte[]转16进制;
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="sourcedata">密文,16进制</param>
        /// <param name="HexPrivatekey">16进制私钥</param>
        /// <returns></returns>
        public string Decrypt(string sourcedata, string HexPrivatekey)
        {
            //私钥,Hex转Base64
            string privatekey = RSAPublicKeyJava2DotNet(HexPrivatekey);
            //密文,Hex转byte[]
            byte[] AInput = StringHelper.HexToByte(sourcedata); 

            RSACryptoServiceProvider oRSA2 = new RSACryptoServiceProvider();
            oRSA2.FromXmlString(privatekey);
            byte[] AOutput = oRSA2.Decrypt(AInput, false);
            return Encoding.UTF8.GetString(AOutput);////byte[]转string;
        }
        #endregion
    }
}
