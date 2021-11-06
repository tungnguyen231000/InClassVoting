$(document).ready(function () {
	console.log('validate');
	$("#fillblank-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"questionText": {
				required: true
			},
			"answer": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"questionText": {
				required: "*Question is required"
			},
			"answer": {
				required: "*Answer is required"
			}
		}
	});

	$('#checkbox-form').validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		groups: {
			nameGroup: "cbOption option"
		},
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"questionText": {
				required: true
			},
			"cbOption": {
				required: true
			},
			"option": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"questionText": {
				required: "*Question is required"
			},
			"cbOption": {
				required: "*Checkbox is required"
			},
			"option": {
				required: "*Option is required"
			}
		},
		errorPlacement: function (error, element) {
			if (element.attr("name") == "cbOption" || element.attr("name") == "option") {
				error.insertAfter(".ms-error");
			} else {
				error.insertAfter(element);
			}
		}

	});

	$('#reading-form').validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		groups: {
			nameGroup: "cbOption option"
		},
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"paragraph": {
				required: true
			},
			"questionText": {
				required: true
			},
			"cbOption": {
				required: true
			},
			"option": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"paragraph": {
				required: "*Paragraph is required"
			},
			"questionText": {
				required: "*Question is required"
			},
			"cbOption": {
				required: "*Checkbox is required"
			},
			"option": {
				required: "*Option is required"
			}
		},
		errorPlacement: function (error, element) {
			if (element.attr("name") == "cbOption" || element.attr("name") == "option") {
				error.insertAfter(".ms-error");
			} else {
				error.insertAfter(element);
			}
		}

	});

	$("#matching-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		groups: {
			nameGroup: "txt-left txt-right"
		},
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"matching-left": {
				required: true
			},
			"matching-right": {
				required: true
			},
			"txt-left": {
				required: true
			},
			"txt-right": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"matching-left": {
				required: "*Matching question is required"
			},
			"matching-right": {
				required: "*Matching question is required"
			},
			"txt-left": {
				required: "*Option left is required"
			},
			"txt-right": {
				required: "*Option right is required"
			},
		},
		errorPlacement: function (error, element) {
			if (element.attr("name") == "txt-left" || element.attr("name") == "txt-right") {
				error.insertAfter(".ms-error");
			} else {
				error.insertAfter(element);
			}
		}
	});

});