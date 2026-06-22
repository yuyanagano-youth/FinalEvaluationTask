using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using Task1.model;
using static Microsoft.Data.SqlClient.Internal.SqlClientEventSource;
using static System.Net.WebRequestMethods;

namespace Task1.Repositories
{
    public class TaskRepository
    {

        private readonly string _connectionString;

        // コンストラクター — ASP.NET Core がこれを呼び出し、IConfiguration を渡します。
        // IConfiguration は appsettings.json を自動的に読み取ります。
        public TaskRepository(IConfiguration config)
        {
            // GetConnectionString("DefaultConnection") は次の値を読み取ります:
            //   appsettings.json → "ConnectionStrings" → "DefaultConnection"
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "appsettings.json に DefaultConnection が見つかりません");
        }

        private SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();   // 実際にSQL Serverに接続しています
            return connection;
        }

        public List<TaskModels> GetAll(string? filter)
        {
            List<TaskModels> result;
            // 接続を開始する — "using"で自動的に閉じられる
            using SqlConnection connection = OpenConnection();

            // SQLを構築する
            string sql;
            {
            sql = @"	
            SELECT *	
            FROM   TASKS";
            }

            var allTasks = connection.Query<TaskModels>(sql).ToList();

            if (filter == "未着手")
            {
                result = allTasks.Where(e => e.STATUS == "未着手").ToList();
            }
            else if(filter == "進行中")
            {
                result = allTasks.Where(e => e.STATUS == "進行中").ToList();
            }
            else if (filter == "完了")
            {
                result = allTasks.Where(e => e.STATUS == "完了").ToList();
            }
            else
            {
                result = allTasks;
            }

            return result;
        }

        public List<TaskModels> PostSearch(string assignee)
        {
            // 接続を開始する — "using"で自動的に閉じられる
            using SqlConnection connection = OpenConnection();

            // SQLを構築する
            string sql;
            {
            sql = @"
            SELECT *	
            FROM   TASKS
            WHERE ASSIGNEE = @Assignee";
            }
            return connection.Query<TaskModels>(sql, new {Assignee = assignee}).ToList();
        }

        public TaskModels? GetById(int id)
        {
            // 接続を開始する — "using"で自動的に閉じられる
            using SqlConnection connection = OpenConnection();

            // SQLを構築する
            string sql;

            sql = @"	
            SELECT * 
            FROM TASKS
            WHERE TASK_ID = @Id
            ";

            return connection.QueryFirstOrDefault<TaskModels>(sql, new { Id = id });
        }

        public void Update(TaskModels task)
        {
            using SqlConnection connection = OpenConnection();

            string sql = @"
            UPDATE  TASKS
            SET     TASK_NAME    = @TASK_NAME,
                    ASSIGNEE   = @ASSIGNEE,
                    DUE_DATE  = @DUE_DATE,
                    STATUS = @STATUS,
                    UPDATE_DATETIME = GETDATE()	

            WHERE   TASK_ID = @TASK_ID";

            connection.Execute(sql, new
            {
                task.TASK_NAME,
                task.ASSIGNEE,
                task.DUE_DATE,
                task.STATUS,
                task.TASK_ID
            });
        }
    }
}
