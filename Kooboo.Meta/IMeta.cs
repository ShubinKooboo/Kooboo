﻿using Kooboo.Meta.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Meta
{
    public interface IMeta
    {
        string SetRoute(string name);

        View AddView(View view);

    }
}
