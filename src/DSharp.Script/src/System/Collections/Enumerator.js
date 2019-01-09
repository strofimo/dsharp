function Enumerator(obj, keys) {
    var index = -1;
    var length = keys
        ? keys.length
        : obj.length;
    var lookup = keys
        ? function () {
            return { key: keys[index], value: obj[keys[index]] };
        }
        : function () {
            return obj[index];
        };

    this.current = null;
    this.moveNext = function () {
        index++;
        this.current = lookup();
        return index < length;
    };
    this.reset = function () {
        index = -1;
        this.current = null;
    };
}

var _nopEnumerator = {
    current: null,
    moveNext: function () {
        return false;
    },
    reset: _nop
};

function enumerate(o) {
    if (!isValue(o)) {
        return _nopEnumerator;
    }
    if (o.getEnumerator) {
        return o.getEnumerator();
    }
    if (o.length !== undefined) {
        return new Enumerator(o);
    }
    return new Enumerator(o, keys(o));
}