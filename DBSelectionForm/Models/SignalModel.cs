using System;
using System.Collections.Generic;
using System.Text;

namespace DBSelectionForm.Models
{
    class SignalModel : ICloneable
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Category { get; set; }

        public void SetPropOnFindDataInDB( int NewValue, string Status, string Category, string Date)
        {
            this.NewValue = NewValue;
            this.Status = Status;
            this.Category = Category;
            this.Date = Date;
        }
        public object Clone()
        {
            return new SignalModel { Name = this.Name, Category = this.Category, Date = this.Date, NewValue = this.NewValue, OldValue = this.OldValue, Status = this.Status };
        }
    }
}
