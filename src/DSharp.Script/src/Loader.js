"use strict";

(function (global) {
  function _ss() {
    {{body}}

    return extend(module('ss', null, {
        IServiceProvider: defineInterface(IServiceProvider),
        IDisposable: defineInterface(IDisposable),
        IEnumerable: defineInterface(IEnumerable),
        IEquatable_$1: defineInterface(IEquatable_$1),
        IComparable_$1: defineInterface(IComparable_$1),
        IEnumerable_$1: defineInterface(IEnumerable_$1, [IEnumerable]),
        IEnumerator: defineInterface(IEnumerator),
        IEnumerator_$1: defineInterface(IEnumerator_$1, [IEnumerator]),
        ICollection: defineInterface(ICollection, [IEnumerable]),
        ICollection_$1: defineInterface(ICollection_$1, [IEnumerable_$1, IEnumerable]),
        IReadOnlyCollection_$1: defineInterface(IReadOnlyCollection_$1, [IEnumerable_$1, IEnumerable]),
        IComparer: defineInterface(IComparer),
        IComparer_$1: defineInterface(IComparer_$1),
        IEqualityComparer: defineInterface(IEqualityComparer),
        IEqualityComparer_$1: defineInterface(IEqualityComparer_$1),
        Dictionary_$2: defineClass(Dictionary_$2, {}, [], null, [IDictionary, IDictionary_$2, IReadOnlyDictionary_$2]),
        IDictionary: defineInterface(IDictionary, [IEnumerable]),
        IDictionary_$2: defineInterface(IDictionary_$2, [IEnumerable_$1, IEnumerable]),
        IReadOnlyDictionary_$2: defineInterface(IReadOnlyDictionary_$2, [IEnumerable_$1, IEnumerable]),
        List_$1: defineClass(List_$1, Array, [], null, [IList, IList_$1, IReadOnlyList_$1]),
        IList: defineInterface(IList, [ICollection]),
        IList_$1: defineInterface(IList_$1, [ICollection_$1]),
        IReadOnlyList_$1: defineInterface(IReadOnlyList_$1, [IReadOnlyCollection_$1]),
        EventArgs: defineClass(EventArgs, {}, [], null),
        CancelEventArgs: defineClass(CancelEventArgs, {}, [], null),
        StringBuilder: defineClass(StringBuilder, StringBuilder$, [], null),
        Stack: defineClass(Stack, Stack$, [], null),
        Queue: defineClass(Queue, Queue$, [], null),
        Guid: defineClass(Guid, Guid$, [], null),
        DateTime: defineClass(DateTime, {}, [IEquatable_$1, IComparable_$1], null),
        Lazy: defineClass(Lazy, {}, [], null),
        Nullable: defineClass(Nullable, Nullable$, [], null),
        Enum: defineClass(Enum, {}, [], null),
        MemberType: new Enum('MemberType', {
            all: 191,
            constructor: 1,
            custom: 64,
            event: 2,
            field: 4,
            method: 8,
            nestedType: 128,
            property: 16,
            typeInfo: 32
        })
    }),
    {
        version: '{{version}}',
        isValue: isValue,
        value: value,
        extend: extend,
        keys: keys,
        values: values,
        keyCount: keyCount,
        keyExists: keyExists,
        clearKeys: clearKeys,
        enumerate: enumerate,
        array: toArray,
        toArray: toArray,
        remove: removeItem,
        boolean: parseBoolean,
        regexp: parseRegExp,
        number: parseNumber,
        date: parseDate,
        truncate: truncate,
        now: now,
        today: today,
        compareDates: compareDates,
        error: error,
        string: string,
        emptyString: emptyString,
        whitespace: whitespace,
        format: format,
        setFormatter: setFormatter,
        commaFormatNumber: commaFormatNumber,
        compareStrings: compareStrings,
        startsWith: startsWith,
        endsWith: endsWith,
        padLeft: padLeft,
        padRight: padRight,
        trim: trim,
        trimStart: trimStart,
        trimEnd: trimEnd,
        insertString: insertString,
        removeString: removeString,
        replaceString: replaceString,
        bind: bind,
        baseBind: baseBind,
        bindAdd: bindAdd,
        bindSub: bindSub,
        bindExport: bindExport,
        paramsGenerator: paramsGenerator,
        namedFunction: namedFunction,
        createPropertyGet: createPropertyGet,
        createPropertySet: createPropertySet,
        createReadonlyProperty: createReadonlyProperty,
        defineProperty: defineProperty,
        copyArray: copyArray,

        module: module,
        modules: _modules,

        isClass: isClass,
        isInterface: isInterface,
        typeOf: typeOf,
        type: type,
        typeName: typeName,
        canCast: canCast,
        safeCast: safeCast,
        canAssign: canAssign,
        instanceOf: instanceOf,
        baseProperty: baseProperty,
        defineClass: defineClass,
        defineInterface: defineInterface,
        getConstructorParams: getConstructorParams,
        createInstance: paramsGenerator(1, createInstance),
        getMembers: getMembers,
        getGenericTemplate: getGenericTemplate,
        makeGenericType: makeGenericType,

        culture: culture,
        fail: fail,
        contains: contains,
        insert: insert,
        clear: clear,
        addRange: addRange,
        getItem: getItem,
        setItem: setItem,
        removeAt: removeAt,
        removeItem: removeItem,
        addKeyValue: addKeyValue,
        createGenericType: createGenericType,
        getGenericConstructor: getGenericConstructor,
        getTypeArgument: getTypeArgument,
        initializeObject: initializeObject,
    });
  }


  function _export() {
    var ss = _ss();
    typeof exports === 'object' ? ss.extend(exports, ss) : global.ss = ss;
  }

  global.define ? global.define('ss', [], _ss) : _export();
})(this);