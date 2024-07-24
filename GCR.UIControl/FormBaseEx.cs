using System.Reflection;
using System.Text;

namespace GCR.UIControl
{
    public static class FormBaseEx
    {
        public static void SetData<T>(this FormBase f, T data)
        {
            List<Field> resolvedFields = ResolveFormFields(f);
            if (resolvedFields.Count == 0 || data == null) return;
            SetDataByFields(resolvedFields, JObject.FromObject(data));
        }

        /// <summary>
        /// 从给定的<see cref="FormBase"/>实例中获取类型为<T>的序列化数据。
        /// </summary>
        /// <param name="f">要从中提取数据的表单基础实例。</param>
        /// <typeparam name="T">要转换的目标类型。</typeparam>
        /// <returns>一个可空的<T>对象，表示从表单字段序列化的数据。</returns>
        public static T? GetData<T>(this FormBase f)
        {
            List<Field> resolvedFields = ResolveFormFields(f);
            var jobj = GetDataByFields(resolvedFields);
            return jobj.ToObject<T>();
        }

        /// <summary>
        /// 从给定的<see cref="FormBase"/>实例中获取JSON对象表示的数据。
        /// </summary>
        /// <param name="f">要从中提取数据的表单基础实例。</param>
        /// <returns>一个包含表单字段数据的JSON对象。</returns>
        public static JObject GetData(this FormBase f)
        {
            List<Field> resolvedFields = ResolveFormFields(f);
            var jobj = GetDataByFields(resolvedFields);
            return jobj;
        }

        /// <summary>
        /// 根据提供的字段列表创建一个JSON对象，其中包含每个字段的ID和文本值。
        /// </summary>
        /// <param name="resolvedFields">要处理的字段列表。</param>
        /// <returns>一个包含字段ID和文本值的JSON对象。</returns>
        private static JObject GetDataByFields(List<Field> resolvedFields)
        {
            var jobj = new JObject();
            foreach (Field field in resolvedFields)
            {
                string text = "";
                if (field is CheckBoxList)
                {
                    text = String.Join(", ", (field as CheckBoxList).SelectedValueArray);
                }
                else if (field is RadioButtonList)
                {
                    text = (field as RadioButtonList).SelectedValue;
                }
                else
                {
                    // 获取 Text 属性的值
                    var propertyInfo = field.GetType().GetProperty("Text", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (propertyInfo != null)
                    {
                        text = propertyInfo.GetValue(field).ToString();
                    }
                }

                if (!String.IsNullOrEmpty(text))
                {
                    jobj.Add(field.ID, text);
                }
            }
            return jobj;
        }

        private static void SetDataByFields(List<Field> resolvedFields, JObject jobj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Field field in resolvedFields)
            {
                if (!jobj.ContainsKey(field.ID)) continue;
                string text = jobj[field.ID]?.ToString() ?? "";
                if (field is CheckBoxList)
                {
                    sb.Append($"F.ui['{field.ID}'].setValue([{text}]);");
                }
                else if (field is RadioButtonList)
                {
                    sb.Append($"F.ui['{field.ID}'].setValue([{text}]);");
                }
                else //if (field is RealTextField)
                {
                    // 获取 Text 属性的值
                    //var propertyInfo = field.GetType().GetProperty("Text", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //if (propertyInfo != null && propertyInfo.CanWrite)
                    //{
                    //    //text = propertyInfo.GetValue(field).ToString();
                    //    propertyInfo.SetValue(field, text);
                    //}
                    sb.Append($"F.ui['{field.ID}'].setText('{text}');");
                }
                FineUICore.PageContext.RegisterStartupScript(sb.ToString());
            }
        }

        /// <summary>
        /// 递归地解析<see cref="ControlBase"/>及其子控件，收集所有字段。
        /// </summary>
        /// <param name="control">要解析的控件基础实例。</param>
        /// <returns>一个包含所有找到的字段的列表。</returns>
        private static List<Field> ResolveFormFields(ControlBase control)
        {
            var resolvedFields = new List<Field>();
            if (control is PanelBase)
            {
                if (control is FineUICore.Form)
                {
                    foreach (FormRow row in (control as FineUICore.Form).Rows)
                    {
                        foreach (ControlBase rowItem in row.Items)
                        {
                            resolvedFields.AddRange(ResolveFormFields(rowItem));
                        }
                    }
                }

                foreach (ControlBase item in (control as PanelBase).Items)
                {
                    resolvedFields.AddRange(ResolveFormFields(item));
                }
            }
            else if (control is Field)
            {
                resolvedFields.Add(control as Field);
            }
            return resolvedFields;
        }
    }
}
