﻿using Dapper;
using Discount.Grps.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grps.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var connection = OpenDbConnection())
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                    "SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

                return coupon ?? new Coupon() { ProductName = "No Discount", Description = "No Description Desc" };
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = OpenDbConnection())
            {
                var affected =await connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                return affected != 0;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = OpenDbConnection())
            {
                var affected = await connection.ExecuteAsync
                    ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

                return affected != 0;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = OpenDbConnection())
            {
                var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

                return affected != 0;

            }
        }

        private IDbConnection OpenDbConnection()
        {
            return new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
    }
}
