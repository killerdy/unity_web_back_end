const sql = require('mysql');
const databaseconfig = require('./config.js');
module.exports = {
    query: function (sql_query, params, callback) {
        let connection = sql.createConnection(databaseconfig);
        connection.connect(function (err) {
            if (err) {
                console.log("数据库连接失败");
                throw err;
            }
            connection.query(sql_query,params,function(err,results,fields){
                if(err)
                {
                    console.log("数据操作失败");
                    throw err;
                }
                // console.log(sql_query);
                // console.log(params);
                // console.log(results);
                callback&&callback(results,fields);
                connection.end(function(err){
                    if(err){
                        console.log("关闭数据库失败");
                        throw err;
                    }
                });
            });
        });

    }
};