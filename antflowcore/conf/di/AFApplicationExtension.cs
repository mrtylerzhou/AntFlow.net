using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Microsoft.Extensions.DependencyInjection;
public static class AFApplicationExtension
{
    public static IMvcBuilder AddAFApplicationComponents(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(AFApplicationExtension).Assembly);
        return builder;
    }
}
