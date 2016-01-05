using System.Collections.Generic;

namespace jqGridToPdfReportMvcApp.Models
{
    /*
        { "groupOp": "AND",
              "groups" : [ 
                { "groupOp": "OR",
                    "rules": [
                        { "field": "name", "op": "eq", "data": "England" }, 
                        { "field": "id", "op": "le", "data": "5"}
                     ]
                } 
              ],
              "rules": [
                { "field": "name", "op": "eq", "data": "Romania" }, 
                { "field": "id", "op": "le", "data": "1"}
              ]
        }      
     */

    public class SearchFilter
    {
        public string groupOp { set; get; }
        public List<SearchGroup> groups { set; get; }
        public List<SearchRule> rules { set; get; }
    }

    public class SearchRule
    {
        public string field { set; get; }
        public string op { set; get; }
        public string data { set; get; }

        public override string ToString()
        {
            return string.Format("'{0}' {1} '{2}'", field, op, data);
        }
    }

    public class SearchGroup
    {
        public string groupOp { set; get; }
        public List<SearchRule> rules { set; get; }
    }
}