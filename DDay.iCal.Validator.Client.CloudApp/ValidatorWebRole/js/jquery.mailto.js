jQuery.fn.mailto = function() {
	return this.each(function(){
		var email = $(this).html().replace(/\s*\(.+\)\s*/, "@");
		$(this).before('<a href="mailto:' + email + '" rel="nofollow" title="Email ' + email + '">' + email + '</a>').remove();
	});
};