# MINIMAL API FOR CONNECTION.
from connection import initialize_connection

resp_get = initialize_connection("https://pokeapi.co/api/v2/pokemon/ditto", "get", 0)

resp_post = initialize_connection("https://reqres.in/api/users", "post", 0, json={"name": "morpheus", "job": "leader"})

print(resp_post.json())