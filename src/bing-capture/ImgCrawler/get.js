const request = require('request');
const fs = require('fs');
const path = require('path');

var start=function (){
    request.get('http://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=30&mkt=zh-CN', (error, response, body) => {
        var bodyjson=JSON.parse(body);
        bodyjson.images.forEach(img => {
            const arr = img.url.split('/');
            const str = arr[arr.length - 1];
            if(!fs.existsSync(path.join('./img', str))){
                console.log(img.url);
                request("http://cn.bing.com/"+img.url).pipe(fs.createWriteStream(path.join('./img', str)));
                console.log(`${new Date()}${str} is ok!`);
            }
            else{
                console.log(str+" 已存在");
            }
        });
    });
    
    request.get('http://cn.bing.com/HPImageArchive.aspx?format=js&idx=1&n=30&mkt=zh-CN', (error, response, body) => {
        var bodyjson=JSON.parse(body);
        bodyjson.images.forEach(img => {
            const arr = img.url.split('/');
            const str = arr[arr.length - 1];
            if(!fs.existsSync(path.join('./img', str))){
                console.log(img.url);
                request("http://cn.bing.com/"+img.url).pipe(fs.createWriteStream(path.join('./img', str)));
                console.log(`${new Date()}${str} is ok!`);
            }
            else{
                console.log(str+" 已存在");
            }
        });
       
    });
};

exports.start=start;