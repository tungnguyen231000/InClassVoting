$(document).ready(function () {
	/*console.log('validate');*/
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
			nameGroup: "answerLeft answerRight"
		},
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"answerLeft": {
				required: true
			},
			"answerRight": {
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
			"answerLeft": {
				required: "*Option left is required"
			},
			"answerRight": {
				required: "*Option right is required"
			},
		},
		errorPlacement: function (error, element) {
			if (element.attr("name") == "answerLeft" || element.attr("name") == "answerRight") {
				error.insertAfter(".ms-error");
			} else {
				error.insertAfter(element);
			}
		}
	});

	
});