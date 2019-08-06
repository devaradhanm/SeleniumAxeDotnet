var callback = arguments[arguments.length - 1];

// arguments[0] will have this script content
var context = typeof arguments[0] === 'string' ? JSON.parse(arguments[0]) : arguments[0];
context = context || document;

var options = JSON.parse(arguments[1]);

console.log("options passed - ", JSON.stringify(options));
console.log("context passed - ", context);

var result = { error: '', results: null };

axe.run(context, options, function (err, res) {
    {
        if (err) {
            result.error = err.message;
        } else {
            result.results = res;
        }
        callback(JSON.stringify(result));
    }
});