using System;
using System.Data.SqlClient;

namespace TimeReports
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ExampleDb;Integrated Security=True;MultipleActiveResultSets = True";

            #region SQL
            string sqlTimeReports = "SELECT * FROM time_reports";

            string sqlNumberEmployees = "SELECT COUNT(*) FROM employees";

            string sqlAllEmployees = "SELECT * FROM employees";
            #endregion

            int[][] employeeDays = new int[7][];

            double[][] employeeHours = new double[7][];

            string[] weekDays = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            int s = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand commandA = new SqlCommand(sqlNumberEmployees, connection);
                object p = commandA.ExecuteScalar();

                int k = (int)p;

                for (int i = 0; i < 7; i++)
                {
                    employeeDays[i] = new int[k];

                    employeeHours[i] = new double[k];
                }

                string[] employeesNames = new string[k];

                SqlCommand commandB = new SqlCommand(sqlAllEmployees, connection);
                SqlDataReader readerB = commandB.ExecuteReader();

                while (readerB.Read())
                {
                    employeesNames[s] = readerB.GetString(1);
                    s++;
                }

                SqlCommand command = new SqlCommand(sqlTimeReports, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DateTime dateValue = reader.GetDateTime(3);

                    int day = (int)dateValue.DayOfWeek;

                    if (day == 0)
                    {
                        day += 6;
                    }
                    else
                    {
                        day -= 1;
                    }


                    if (employeeDays[day][(reader.GetInt32(1) - 1)] == 0)
                    {
                        employeeDays[day][(reader.GetInt32(1) - 1)] = 1;
                    }
                    else
                    {
                        employeeDays[day][(reader.GetInt32(1) - 1)] += 1;
                    }

                    
                    if (employeeHours[day][(reader.GetInt32(1) - 1)] == 0)
                    {
                        employeeHours[day][(reader.GetInt32(1) - 1)] = reader.GetDouble(2);
                    }
                    else
                    {
                        employeeHours[day][(reader.GetInt32(1) - 1)] += reader.GetDouble(2);
                    }
                }

                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < k; j++)
                        employeeHours[i][j] = Math.Round(employeeHours[i][j] / employeeDays[i][j], 2, MidpointRounding.AwayFromZero);
                }

                string[] result = new string[7];

                double max1 = 0, max2 = 0, max3 = 0;
                string Max1=" ", Max2 = " ", Max3 = " ";

                for (int i = 0; i < 7; i++)
                {
                    max1 = 0; max2 = 0; max3 = 0;

                    Max1 = " "; Max2 = " "; Max3 = " ";

                    for (int j = 0; j < k; j++) 
                    {
                        if (employeeHours[i][j] > max1)
                        {
                            max3 = max2;
                            max2 = max1;

                            Max3 = Max2;
                            Max2 = Max1;

                            max1 = employeeHours[i][j];
                            Max1 = $"{employeesNames[j]}({max1} hours),";
                        }
                        else if (employeeHours[i][j] > max2)
                        {
                            max3 = max2;

                            Max3 = Max2;

                            max2 = employeeHours[i][j];
                            Max2 = $"{employeesNames[j]}({max2} hours),";
                        }
                        else if (employeeHours[i][j] > max3)
                        {
                            max3 = employeeHours[i][j];
                            Max3 = $"{employeesNames[j]}({max3} hours)";
                        }
                    }
                    
                    result[i] = $"{Max1}{Max2}{Max3}";

                  //  Console.Write("{0,9}", weekDays[i]);

                    Console.WriteLine("|{0,9}| {1}|", weekDays[i],result[i]);
                }
            }
            Console.Read();  
        }
    }
}
