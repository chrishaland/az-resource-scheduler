export async function cqrs(path, request, token) {
    return await fetch(path, { 
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + token,
        },
        body: JSON.stringify(request || {})
      });
}