using System;
using System.Collections.Generic;
using System.Text;

namespace DBSelectionForm.Models
{
    class SignalModel : ICloneable
    {
        public int Number { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Category { get; set; }
        public bool IsInvariable { get; set; }
        public string WriteDataToFile()
        {
            return $"{NewValue};{Name};{Category};{Status};{Date};{IsInvariable}";
        }
        public void SetPropOnFindDataInDB( object NewValue, string Status, string Category, string Date)
        {
            this.NewValue = NewValue;
            this.Status = Status;
            this.Category = Category;
            this.Date = Date;
        }
        public object Clone()
        {
            return new SignalModel { Name = this.Name, Category = this.Category, Date = this.Date, NewValue = this.NewValue, OldValue = this.OldValue, Status = this.Status, IsInvariable = this.IsInvariable };
        }
    }
}
