using System;
using System.Collections.Generic;
using System.Text;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

public interface IDialogResult<TResult>
{   
    TResult? Value { get; }
}
