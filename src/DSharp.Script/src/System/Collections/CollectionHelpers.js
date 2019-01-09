function toArray(obj) {
    return obj
        ? typeof obj == "string"
            ? JSON.parse("(" + obj + ")")
            : Array.prototype.slice.call(obj)
        : null;
}
function removeItem(a, item) {
    var index = a.indexOf(item);
    return index >= 0
        ? (a.splice(index, 1), true)
        : false;
}
function clearKeys(obj) {
    for (var key in obj) {
        delete obj[key];
    }
}
function keyExists(obj, key) {
    return obj[key] !== undefined;
}
function keys(obj) {
    if (Object.keys) {
        return Object.keys(obj);
    }
    var keys = [];
    for (var key in obj) {
        keys.push(key);
    }
    return keys;
}
function keyCount(obj) {
    return keys(obj).length;
}