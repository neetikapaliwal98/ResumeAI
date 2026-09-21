using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAI.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
