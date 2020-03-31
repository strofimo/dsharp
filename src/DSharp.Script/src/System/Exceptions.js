// Exception

function Exception(userMessage) {
    Error.call(this, userMessage);
    this.name = typeName(typeOf(this));
    this.message = userMessage;
    var implementation = new Error(userMessage);
    implementation.name = this.name;
    Error.captureStackTrace(this, typeOf(this));
}
var Exception$ = {

};

// NotImplementedException

function NotImplementedException() {
    Exception.call(this);
}
var NotImplementedException$ = {

};