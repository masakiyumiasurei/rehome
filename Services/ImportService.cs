using Dapper;
using Microsoft.Data.SqlClient;
using X.PagedList;
using System.Data;
using rehome.Models.DB;
using rehome.Enums;
using rehome.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace rehome.Services
{
    public interface IImportService
    {       
        void InsertData(List<商品> imports);
                
    }

    public class ImportService : ServiceBase, IImportService
    {
        private readonly ILogger<QuoteService> _logger;

        public ImportService(ILogger<QuoteService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }


        public void InsertData(List<商品> imports)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                
                connection.Open();

                var builder = new SqlBuilder();
                var template = builder.AddTemplate(
                "MERGE INTO T_商品 AS target " +
                "USING(VALUES(@商品ID)) AS source(商品ID) " +
                "ON target.商品ID = source.商品ID " +
                "WHEN MATCHED THEN " +
                "UPDATE SET " +                
                "商品名 = @商品名, " +
                "単価 = @単価, " +
                "単位 = @単位, " +                
                "仕入額 = @仕入額, " +               
                "削除フラグ=@削除フラグ " +
                "WHEN NOT MATCHED THEN " +
                "INSERT(商品名, 単価, 単位, " +
                " 仕入額, 削除フラグ) " +
                "VALUES(@商品名, @単価, @単位," +
                " @仕入額, @削除フラグ); ");

                connection.Execute(template.RawSql, imports); 

            }
        }
 
    }
}


