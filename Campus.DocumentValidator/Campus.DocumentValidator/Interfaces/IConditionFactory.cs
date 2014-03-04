using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Campus.DocumentValidator
{
    public interface IConditionFactory
    {
        IConditionExpression CreateConditionExpression(string TypeName, XmlElement condDescription);
    }
}
