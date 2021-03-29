function getAntiforgeryToken() {
    var name = "CSRF-TOKEN=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

export async function cqrs(path, request) {
    return await fetch(path, { 
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          //'X-CSRF-TOKEN': getAntiforgeryToken()
        },
        body: JSON.stringify(request || {})
      });
}