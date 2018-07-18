let Service = require('node-windows').Service;

let svc = new Service({
    name: 'node_imgcrawler',    //服务名称
    description: '图片爬虫服务(Bing)', //描述
    script: 'D:/WorkSpace/node-js-demo/bing-capture/ImgCrawler/index.js' //nodejs项目要启动的文件路径
});

svc.on('install', () => {
    svc.start();
});

svc.on('uninstall', () => {
    console.log("uninstall complete");
});

if(svc.exists)return svc.uninstall();

svc.install();

