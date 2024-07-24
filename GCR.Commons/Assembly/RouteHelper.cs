using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GCR.Commons
{
    /// <summary>
    /// 路由
    /// </summary>
    public static class RouteHelper
    {
        /// <summary>
        /// 所有的路由路径
        /// </summary>
        public static readonly List<string> AllRoutePath;

        /// <summary>
        /// 路由
        /// </summary>
        static RouteHelper()
        {
            AllRoutePath = GetAllRoutes();
        }

        /// <summary>
        /// 得到所有的路由地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllRoutes()
        {
            var _serviceProvider = PageContext.GetServerByApp<IServiceProvider>();
            var routes = new List<string>();
            var actionDescriptorsProvider = _serviceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
            var actionDescriptors = actionDescriptorsProvider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    string path = GetRoutePath(controllerActionDescriptor);
                    if (!string.IsNullOrEmpty(path))
                    {
                        routes.Add(path);
                    }
                }
            }
            return routes;
        }

        /// <summary>
        /// 通过controller得到路径地址
        /// </summary>
        /// <param name="controllerActionDescriptor"></param>
        /// <returns></returns>
        private static string GetRoutePath(ControllerActionDescriptor controllerActionDescriptor)
        {
            var routeAttributes = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes<RouteAttribute>();
            if (routeAttributes.Any())
            {
                var routeAttribute = routeAttributes.First();
                var routePrefix = routeAttribute.Template.TrimEnd('/');
                var actionRoute = GetActionRoute(controllerActionDescriptor.MethodInfo);
                var controllerName = GetControllerName(controllerActionDescriptor.ControllerTypeInfo);
                return $"/{routePrefix.Replace("[controller]", controllerName)}/{actionRoute}";
            }
            else
            {
                var area = GetArea(controllerActionDescriptor.ControllerTypeInfo);
                if (!string.IsNullOrEmpty(area))
                {
                    return $"/{area}/{controllerActionDescriptor.ControllerName}/{controllerActionDescriptor.ActionName}";
                }
                else
                {
                    return $"/{controllerActionDescriptor.ControllerName}/{controllerActionDescriptor.ActionName}";
                }
            }
        }

        /// <summary>
        /// 得到路由的配置
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static string GetActionRoute(MethodInfo methodInfo)
        {
            var routeAttributes = methodInfo.GetCustomAttributes<RouteAttribute>();
            if (routeAttributes.Any())
            {
                return routeAttributes.First().Template;
            }
            else
            {
                return methodInfo.Name;
            }
        }

        /// <summary>
        /// 得到controller名字
        /// </summary>
        /// <param name="controllerTypeInfo"></param>
        /// <returns></returns>
        private static string GetControllerName(TypeInfo controllerTypeInfo)
        {
            return controllerTypeInfo.Name.Replace("Controller", "");
        }

        /// <summary>
        /// 得到区域
        /// </summary>
        /// <param name="controllerTypeInfo"></param>
        /// <returns></returns>
        private static string GetArea(TypeInfo controllerTypeInfo)
        {
            var areaAttribute = controllerTypeInfo.GetCustomAttribute<AreaAttribute>();
            if (areaAttribute != null)
            {
                return areaAttribute.RouteValue;
            }
            else
            {
                return "";
            }
        }
    }
}
