//using System.Linq;
//using Autofac;
//using Siemplify.Server.Common.Extensibility;
//using Siemplify.Server.WebApi.Validation;

//namespace Siemplify.Server.WebApi
//{
//    public class Bootstraper : ServerModule
//    {

//        protected override void Load(ContainerBuilder builder)
//        {
//            builder.RegisterType<ValidatorFactory>();
//            RegisterValidators(builder);
//        }

//        protected override string ModuleName
//        {
//            get { return "Web Host"; }
//        }




//        private static void RegisterValidators(ContainerBuilder builder)
//        {
//            var validators = typeof (Bootstraper).Assembly.GetTypes()
//                .Where(x => typeof (FluentValidation.IValidator).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

//            foreach (var validator in validators)
//            {
//                builder.RegisterType(validator).AsImplementedInterfaces().InstancePerLifetimeScope();
//            }
//        }

//    }
//}