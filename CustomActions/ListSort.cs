using Ringhel.Procesio.Action.Core;
using Ringhel.Procesio.Action.Core.ActionDecorators;
using Ringhel.Procesio.Action.Core.Models;
using Ringhel.Procesio.Action.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ncd.ActionLib.Actions
{
    [ClassDecorator(Name = "List Sort", Shape = ActionShape.Circle, Description = "Sorting a list in descending or " +
        "ascending order", Classification = Classification.cat1)]
    [Permissions(CanDelete = true, CanDuplicate = true, CanAddFromToolbar = true)]
    public class ListSort : IAction
    {
        #region Options
        private IEnumerable<OptionModel> OrderOptions { get; } = new List<OptionModel>()
        {
           new OptionModel(){ name = "Ascending Order", value = "ASC"},
           new OptionModel(){ name = "Descending Order", value = "DESC"}
        };
        #endregion

        #region Properties
        [FEDecorator(Label = "Input List", Type = FeComponentType.Text, Tab = "Input Tab")]
        [BEDecorator(IOProperty = Direction.Input)]
        [Validator(IsRequired = true)]
        public IEnumerable<object> InputList { get; set; }

        [FEDecorator(Label = "Input Order (A/D)", Type = FeComponentType.Select, Options = "OrderOptions", Tab = "Input Tab")]
        [BEDecorator(IOProperty = Direction.Input)]
        [Validator(IsRequired = true, Expects = ExpectedType.String)]
        public string InputOrder { get; set; }

        [FEDecorator(Label = "Output Sorted List", Type = FeComponentType.Text, Tab = "Output Tab")]
        [BEDecorator(IOProperty = Direction.Output)]
        [Validator(IsRequired = false)]
        public IEnumerable<object> SortedList { get; set; }
        #endregion

        #region Methods
        private bool CheckConvToJObject(object obj)
        {
            try
            {
                ConvertToJObject(obj);
            }
            catch
            {
                //Something went wrong, cannot return a JObj
                return false;
            }
            return true;
        }

        private JObject ConvertToJObject(object obj)
        {
            return JObject.Parse(obj.ToString());
        }

        private IEnumerable<object> Sort(IEnumerable<object> list, string order)
        {
            if (order == "DESC")
            {
                var result = from obj in list
                             orderby obj descending
                             select obj;
                return result;
            }
            else
            {
                var result = from obj in list
                             orderby obj
                             select obj;
                return result;
            }

        }
        #endregion

        #region Execute
        public async Task Execute()
        {
            if (InputList == null)
            {
                throw new System.Exception("Input list was null. Please add items to it.");
            }

            bool hasDataModels = false, hasPrimitives = false;
            foreach (var item in InputList)
            {
                if (hasDataModels && hasPrimitives)
                {
                    throw new System.Exception("The list was not fill right. Check the data types of the items.");
                }

                if (CheckConvToJObject(item)) // Check if the elements from Input List can be converted to JObj
                {
                    hasDataModels = true;
                }
                else
                {
                    hasPrimitives = true;
                }
            }
            if (hasPrimitives) // If the elements from Input List are not data models
            {
                SortedList = Sort(InputList, InputOrder); // Sort Input List 
            }
            else if (hasDataModels)
            {
                List<object> InputListAux = new List<object>();
                foreach (var item in InputList)
                {
                    // Convert to string each item from the Input List and add it to another list
                    InputListAux.Add(item.ToString());
                }
                SortedList = Sort(InputListAux, InputOrder); // Sort Input List after string representation
            }
            else
            {
                throw new System.Exception("Input list was not fill right. You can not add data models " +
                    "and primitives to the same list.");
            }
        }
        #endregion
    }
}
