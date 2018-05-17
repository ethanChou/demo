
window.onload=function(){
    console.log("window onload!");
}

function wapperLoaded(axctrl) {
    console.log("wrapper plugin loaded!");
}

function CustumEventFired(id,event,data){
    console.log(data);
    alert("id:"+id+",event:"+event+"data:"+data)
}

function Hello(){
    var ctrl= document.getElementById("wapper")
    
    addEvent(ctrl,"CustumEventFired",CustumEventFired);
  
    var config={
        Id:"7891023",
        MethodName:"Hello",
        Args:["richmars",23,true]
    }

    console.log(JSON.stringify(config));
    alert(ctrl.Excute(JSON.stringify(config)));

    alert(ctrl.Set("MyName","homingfly"));
}


function Set(){
    var ctrl= document.getElementById("wapper")
  
    var res=ctrl.Set("MyName",false);
   
    console.log(JSON.stringify(res));
    alert(JSON.stringify(res));
}

function Get(){
    //eval('TestRun(1,\'zxm\')');

    var ctrl= document.getElementById("wapper")
  
    var res=ctrl.Get("MyName");
   
    var tag=JSON.parse(res);
    eval(tag.msg);
    
    console.log(res);
    alert(res);
}

function TestRun(id,name){
    alert("evalRun "+id+" ,"+name);
}

function addEvent(ax,ft,callback){
    try {
        var element = ax;
        var type = ft;
        var handler = callback;

        if (element.attachEvent) {
            element.attachEvent("on" + type, handler);
        } else if (element.addEventListener) {
            element.addEventListener(type, handler, false);
        }
        else {
            element[type] = handler;
        }
    } catch (e) {
        console.log("addEvent error " + e);
    }
}
