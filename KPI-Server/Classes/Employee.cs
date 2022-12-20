using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI_Server.Classes
{
    public class Employee : PersonTask
    {
        public int uuid { get; set; }
        public int upid { get; set; }
        public int ueid { get; set; }
        public string position { get; set; }
        public double salary { get; set; }
        public double award { get; set; }

        public Employee()
        {
            uuid = 0;
            upid = 0;
            ueid = 0;
            position = "NULL";
            salary = 0.0;
            award = 0.0;
        }

        public Employee(int uuid, int upid, int ueid, string position, double salary, double award)
        {
            this.uuid = uuid;
            this.upid = upid;
            this.ueid = ueid;
            this.position = position;
            this.salary = salary;
            this.award = award;
        }

        public Employee(Employee employee)
        {
            this.uuid = employee.uuid;
            this.upid = employee.upid;
            this.ueid = employee.ueid;
            this.position = employee.position;
            this.salary = employee.salary;
            this.award = employee.award;
        }
    }
}
