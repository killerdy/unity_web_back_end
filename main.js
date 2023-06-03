const express = require('express');
// const queryString=require('querystring')
const merge = require("./merge");
const sql = require('./sql')
let app = express();

app.post('/unity_login', async (req, res) => {
    let postParams = ""
    req.on('data', function (params) {
        postParams += params;
        var obj = JSON.parse(postParams);
        if ("keys" in obj && "values" in obj)
            obj = merge.merge(obj);
        // console.log(obj);
        sql.query("select * from usertable where phone=?", [obj.phone], function (results, fields) {
            let resultJson = JSON.stringify(results);
            if (resultJson == "[]") {
                console.log("无此用户");
                return res.end("wu");
            }
            let dataJson = results[0];
            console.log(dataJson['qq']);
            if (dataJson['password'] == obj.password)
                return res.end("ok");
            else
                return res.end("wrong");
        })
    })
})

app.post('/unity_regist', async (req, res) => {
    let postParams = ""
    req.on('data', function (params) {
        postParams += params;
        var obj = JSON.parse(postParams);
        if ("keys" in obj && "values" in obj)
            obj = merge.merge(obj);
        sql.query("select * from usertable where phone=?", [obj.phone], function (results, fields) {
            if (JSON.stringify(results) != "[]")
                return res.end("had");
            else sql.query("insert into usertable(username,password,qq,phone) values(?,?,?,?);", [obj.username, obj.password, obj.qq, obj.phone], function (results, fields) {
                return res.end("ok");
            })
        })
    })
})

app.listen(4001);