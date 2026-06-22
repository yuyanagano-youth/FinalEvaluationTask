using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using NLog;

namespace TaskAuditApp
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TASKS;Trusted_Connection=True;";
        static async Task Main(string[] args)
        {
            Logger.Info("タスク統計バッチ処理を開始します。");
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    // Dapperを用いた非同期での全件取得
                    var sql = "SELECT * FROM TASKS";
                    var taskEnumerable = await connection.QueryAsync<TaskModel>(sql);
                    var taskList = new List<TaskModel>(taskEnumerable);
                    // 解析クラスのインスタンス生成
                    var analyzer = new TaskAnalyzer(Logger);
                    // 1. 期限切れチェックの実行
                    analyzer.CheckOverdueTasks(taskList);
                    // 2. 担当者ごとの集計ログ出力
                    analyzer.LogTaskCountByAssignee(taskList);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "バッチ処理中に予期せぬエラーが発生しました。");
            }
            Logger.Info("タスク統計バッチ処理を終了しました。");
        }
    }
}
