﻿// Copyright 2014-2015 Kasper B. Graversen
// 
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StatePrinter.FieldHarvesters
{
    /// <summary>
    /// Reusable helper methods when implementing <see cref="IFieldHarvester"/>
    /// </summary>
    public class HarvestHelper
    {
        internal const string BackingFieldSuffix = ">k__BackingField";

        const BindingFlags flags = BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly;

        /// <summary>
        /// We ignore all properties as they, in the end, will only point to some computated state or other fields.
        /// Hence they do not provide information about the actual state of the object.
        /// </summary>
        public List<SanitiedFieldInfo> GetFields(Type type)
        {
            return GetFields(type, flags)
              .Select(x => new SanitiedFieldInfo(x, SanitizeFieldName(x.Name), (object o) => x.GetValue(o)))
              .ToList();
        }

        /// <summary>
        /// We ignore all properties as they, in the end, will only point to some computated state or other fields.
        /// Hence they do not provide information about the actual state of the object.
        /// </summary>
        public IEnumerable<FieldInfo> GetFields(Type type, BindingFlags flags)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            if (!IsHarvestable(type))
                return Enumerable.Empty<FieldInfo>();

            return GetFields(type.BaseType, flags).Concat(type.GetFields(flags));
        }

        public List<SanitiedFieldInfo> GetFieldsAndProperties(Type type)
        {
            var fieldsAndProps = GetFieldsAndProperties(type, flags)
                .Select(x =>
                    {
                        Func<object, object> valueFetcher;
                        switch (x.MemberType)
                        {
                            case MemberTypes.Field:
                                valueFetcher = o => ((FieldInfo)x).GetValue(o);
                                break;
                            case MemberTypes.Property:
                                valueFetcher = o => ((PropertyInfo)x).GetValue(o, null);
                                break;
                            default:
                                throw new Exception("Membertype not supported.");
                        }
                        return new SanitiedFieldInfo(x, SanitizeFieldName(x.Name), valueFetcher);
                    });

            return fieldsAndProps.ToList();
        }

    /// <summary>
    /// Returns all Membertype.Field and Membertype.Property except backing-fields
    /// </summary>
    public IEnumerable<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags flags)
    {
      if (type == null)
        return Enumerable.Empty<MemberInfo>();

      if (!IsHarvestable(type))
        return Enumerable.Empty<MemberInfo>();

      return
        GetFieldsAndProperties(type.BaseType, flags)
          .Concat(type.GetMembers(flags))
          .Where(x => NonBackingFieldFilter(x) || IndexerFilter(x)); 
    }

    static bool NonBackingFieldFilter(MemberInfo x)
    {
      return x.MemberType == MemberTypes.Field 
        && !x.Name.EndsWith(BackingFieldSuffix);
    }

    static bool IndexerFilter(MemberInfo x)
    {
      var p = x as PropertyInfo;
      return x.MemberType == MemberTypes.Property //
        && p.GetIndexParameters().Length == 0 // no indexed properties
        && p.GetGetMethod(true) != null;  // must have getter
    }

        /// <summary>
        /// Tell if the type makes any sense to dump
        /// </summary>
        public bool IsHarvestable(Type type)
        {
            var typename = type.ToString();
            if (typename.StartsWith("System.Reflection")
                || typename.StartsWith("System.Runtime")
                || typename.StartsWith("System.SignatureStruct")
                || typename.StartsWith("System.Func"))
                return false;
            return true;
        }

        /// <summary>
        /// Replaces the name of properties to remove the k__BackingField nonsense from the name.
        /// </summary>
        public string SanitizeFieldName(string fieldName)
        {
            return fieldName.StartsWith("<")
              ? fieldName.Substring(1).Replace(BackingFieldSuffix, "")
              : fieldName;
        }
    }
}