
var http=require('http');  
var get= require('./get.js');

http.createServer(function(req,res){  
    res.writeHead(200,{'Content-Type':'text/plain'});  
    res.end('hello node.js');  
}).listen(3000,'localhost',function(){  
    console.log('Server running at http://localhost:3000');  
}); 



const schedule = require('node-schedule');

//6个占位符从左到右分别代表：秒、分、时、日、月、周几
//'*'表示通配符，匹配任意，当秒是'*'时，表示任意秒数都触发，其它类推

const rule = new schedule.RecurrenceRule();
rule.dayOfWeek = [0, new schedule.Range(1, 6)];
//rule.hour = 0;
//rule.minute = 0;
rule.second=30;

schedule.scheduleJob(rule, () => {
    console.log("start get img");
    get.start();
});