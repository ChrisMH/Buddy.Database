using System.Collections.Generic;

namespace Buddy.Database.PostgreSql
{
    public class PgParameter
    {
        public delegate bool IncludeCheckDelegate(object[] values);

        public delegate string ValueGeneratorDelegate(ref int fieldIndex, List<object> fieldValues, object[] values);

        public PgParameter(IncludeCheckDelegate includeCheck, ValueGeneratorDelegate valueGenerator, string fieldName, params object[] values)
        {
            IncludeCheck = includeCheck;
            ValueGenerator = valueGenerator;
            FieldName = fieldName;
            Values = new object[values.Length];
            values.CopyTo(Values, 0);
        }

        public IncludeCheckDelegate IncludeCheck { get; private set; }
        public ValueGeneratorDelegate ValueGenerator { get; private set; }
        public string FieldName { get; private set; }
        public object[] Values { get; private set; }
    }
}