using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DDay.iCal.Validator
{
    public class ValidatorActivator
    {
        static Dictionary<Type, ConstructorInfo> _CalendarPlusTextConstructor = new Dictionary<Type,ConstructorInfo>();
        static Dictionary<Type, ConstructorInfo> _TextConstructor = new Dictionary<Type, ConstructorInfo>();
        static Dictionary<Type, ConstructorInfo> _CalendarConstructor = new Dictionary<Type, ConstructorInfo>();


        static public IValidator Create(Type validatorType, IResourceManager mgr, IICalendarCollection calendars)
        {
            return Create(validatorType, mgr, calendars, null);
        }

        static public IValidator Create(Type validatorType, IResourceManager mgr, string iCalendarText)
        {
            return Create(validatorType, mgr, null, iCalendarText);
        }

        static public IValidator Create(Type validatorType, IResourceManager mgr, IICalendarCollection calendars, string iCalendarText)
        {
            IValidator validator = null;

            if (validatorType != null)
            {
                ConstructorInfo ci = null;

                if (iCalendarText != null)
                {                    
                    if (!_CalendarPlusTextConstructor.ContainsKey(validatorType))
                        _CalendarPlusTextConstructor[validatorType] = validatorType.GetConstructor(new Type[] { typeof(IResourceManager), typeof(string), typeof(IICalendarCollection) });
                    ci = _CalendarPlusTextConstructor[validatorType];
                    if (ci != null)
                        validator = ci.Invoke(new object[] { mgr, iCalendarText, calendars }) as IValidator;
                    else
                    {
                        if (!_TextConstructor.ContainsKey(validatorType))
                            _TextConstructor[validatorType] = validatorType.GetConstructor(new Type[] { typeof(IResourceManager), typeof(string) });
                        ci = _TextConstructor[validatorType];
                        if (ci != null)
                            validator = ci.Invoke(new object[] { mgr, iCalendarText }) as IValidator;
                    }
                }
                if (validator == null)
                {
                    if (!_CalendarConstructor.ContainsKey(validatorType))
                        _CalendarConstructor[validatorType] = validatorType.GetConstructor(new Type[] { typeof(IResourceManager), typeof(IICalendarCollection) });
                    ci = _CalendarConstructor[validatorType];
                    if (ci != null)
                        validator = ci.Invoke(new object[] { mgr, calendars }) as IValidator;
                }
            }

            return validator;
        }
    }
}
