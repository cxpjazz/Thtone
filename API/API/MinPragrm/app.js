//app.js
var url=require('/url.js');

var onlogin=url.host+'/Home/OnLogin';

App({
  onLaunch: function () {
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)
  },
  ///微信小程序登录
  OnLogin:function(callback){
    var that=this;
    if (that.globalData.sessionId!=''){
        console.log("读取信息");
        callback(null, that.globalData.sessionId);
    }else{
      wx.showLoading({
        title: '认证中~~~',
      })
        ///获取code
        wx.login({
          success:function(d){
            console.log(d);
            //return false;
              ///请求
            wx.request({
              url: 'https://471625.net/Home/OnLogin',
              method: 'post',
              data: { 'code': d.code },
              success: function (db) {
                if(db.statusCode==200){
                  that.globalData.sessionId=db.data.value;
                  console.log(db);
                  wx.setStorageSync('cookie', db.header["Set-Cookie"]);
                  wx.hideLoading();
                  callback(null, that.globalData.sessionId);
                }else{
                  wx.hideLoading();
                  wx.showModal({
                    title: '提示',
                    content: '网络异常！请重新进入本小程序',
                    showCancel: false
                  })
                }
              },
              fail:function(){
                wx.hideLoading();
                wx.showModal({
                  title: '提示',
                  content: '网络异常！请重新进入本小程序',
                  showCancel: false
                })
              }
            })
              ///请求
          }
        })      
        ///获取code
    }

  },
  globalData: {
    sessionId:''
  },
  data:{
    ////返回的数据
    respone:null,
  },
  post:function(Url,Data,Fn){
    var that=this;
  ////请求
  wx.request({
    url: Url,
    method:'post',
    data:Data,
    success:function(respone){
      if(respone.statusCode==200){
        that.data.respone=respone;
        Fn();
      }else{
        wx.showModal({
          title: '提示',
          content: '请求失败，请重试',
          showCancel:false
        })
      }


    }
  })

  ////请求
  }
})