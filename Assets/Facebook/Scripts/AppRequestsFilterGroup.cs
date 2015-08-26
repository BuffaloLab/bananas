using System;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    public sealed class AppRequestsFilterGroup: Dictionary<string, object>
    {
      public AppRequestsFilterGroup(string name, List<string> user_ids)
      {
        this["name"] = name;
        this["user_ids"] = user_ids;
      }
    }
}
