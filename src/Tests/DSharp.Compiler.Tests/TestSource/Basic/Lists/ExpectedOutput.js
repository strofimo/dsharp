"use strict";

define('sample', ['ss'], function(ss) {
  var $global = this;

  // DSharp.Compiler.Tests.TestSource.Basic.Lists.PulicClass

  function PulicClass() {
    var list = [];
    list.push('one');
    list.push([ 'two', 'three' ]);
    var array = list;
  }
  var PulicClass$ = {

  };


  var $exports = ss.module('sample', null,
    {
      PulicClass: ss.defineClass(PulicClass, PulicClass$, [], null)
    });


  return $exports;
});
