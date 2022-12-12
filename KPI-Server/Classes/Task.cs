namespace KPI_Server.Classes
{
    internal class Task
    {
        public int utid { get; set; }
        public int ueid { get; set; }
        public int uuid { get; set; }
        public int upid { get; set; }
        public string task_text { get; set; }
        public DateOnly task_start_date { get; set; }
        public DateOnly task_end_date { get; set; }
        public int task_count_to_do { get; set; }
        public int task_count_of_completed { get; set; }

        public Task() 
        { 
            utid = 0;
            ueid = 0;   
            uuid = 0;   
            upid = 0;
            task_text = "NULL";
            task_start_date = new DateOnly();
            task_end_date = new DateOnly();
            task_count_to_do = 30;
            task_count_of_completed = 30;
        }

        public Task(int utid, int ueid, int uuid, int upid, string task_text, DateOnly task_start_date, DateOnly task_end_date, int task_count_to_do, int task_count_of_completed)
        {
            this.utid = utid;
            this.ueid = ueid;
            this.uuid = uuid;
            this.upid = upid;
            this.task_text = task_text;
            this.task_start_date = task_start_date;
            this.task_end_date = task_end_date;
            this.task_count_to_do = task_count_to_do;
            this.task_count_of_completed = task_count_of_completed;
        }

        public Task(Task task)
        {
            this.utid = task.utid;
            this.ueid = task.ueid;
            this.uuid = task.uuid;
            this.upid = task.upid;
            this.task_text = task.task_text;
            this.task_start_date = task.task_start_date;
            this.task_end_date = task.task_end_date;
            this.task_count_to_do = task.task_count_to_do;
            this.task_count_of_completed = task.task_count_of_completed;
        }
    }
}
