﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public record ApiResponse(string Message, bool Status,object Result);

    public record ApiResponse<T>(string Message, bool Status, T Result);

}
