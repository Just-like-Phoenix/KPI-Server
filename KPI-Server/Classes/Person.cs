using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI_Server.Classes
{
    public class Person : Employee
    {
        public static Person person;
        public static Person SetPerson(Person p) => person = p;
        public static Person GetPerson() => person;

        public int uuid { get; set; }
        public int upid { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string patronymic { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }

        public Person() 
        {
            uuid = 0;
            upid = 0;
            name = "NULL";
            surname = "NULL";
            patronymic = "NULL";
            email = "NULL";
            telephone = "NULL";
        }

        public Person(int uuid, int upid, string name, string surname, string patronymic, string email, string telephone)
        {
            this.uuid = uuid;
            this.upid = upid;
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.email = email;
            this.telephone = telephone;
        }

        public Person(Person person)
        {
            this.uuid = person.uuid;
            this.upid = person.upid;
            this.name = person.name;
            this.surname = person.surname;
            this.patronymic = person.patronymic;
            this.email = person.email;
            this.telephone = person.telephone;
        }
    }
}
