using System;

namespace Business.Models
{
    public class QuestionAnswerModel
    {
        public Guid CustomerId { get; set; }

        public Guid FormId { get; set; }

        public Guid AuditId { get; set; }

        public string Category { get; set; }

        public string Question { get; set; }

        public string KeyPhrazes { get; set; }

        public string Answer { get; set; }
    }
}