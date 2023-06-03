const http = require('http');
const queryString = require('querystring');
const fs = require('fs');
const mime = require('mime');
const path = require('path');
const connection = require('./sql');


let sever = http.createServer();
sever.on('request', function (request, response) {
    let postParams = "";
    console.log(request);
    request.on('data', function (params) {
        postParams = postParams + params;
        console.log(params);
        
    })
    request.on('end', function () {
        var username, password;
        username = queryString.parse(postParams).username;
        password = queryString.parse(postParams).password;
        console.log(postParams);
        connection.query("select * from usertable where username=?", [username], function (results, fields) {
            let resultJson = JSON.stringify(results);
            if (resultJson == '[]') {
                console.log("查无此用户");
                return response.end('<h1>无此用户,需要注册</h1>');
            }
            let dataJson = JSON.parse(resultJson);
            let name = dataJson[0].username;
            let pwd = dataJson[0].password;
            if (pwd == password && name == username) {
                console.log("密码正确");
                findPage('/success.html', response);
            }
            else {
                console.log("密码错误");
                response.end('<h1>密码错误！</h1>')
            }
        });
    })
}).listen(4001);
function findPage(url, res) {
    __dirname += "\\public";
    const static = path.join(__dirname, url);
    let fileType = mime.getType(static);
    console.log(static);
    fs.readFile(static, function (err, result) {
        if (!err) {
            res.end(result);
        }
    })
}


