# Assuming we are only using the GET method.
from requests import get as request_get, post as request_post, exceptions, Response
from time import sleep

# define constants.
SUCCESFUL_CODE_GET = 200
SENT_MESSAGE_CODE = 201

# save those codes here because we don't want to put if-else thingy.
success_codes = [
    SUCCESFUL_CODE_GET, SENT_MESSAGE_CODE
]

def create_connection(url : str, mode : str, **kwargs : ...) -> Response:
    """
    Creates a connection with the specified HTTP Method.
    Args:
        url (str): The specified URL to connect.
        mode (str): The HTTP method (only GET and POST).
        kwargs (vararg): The parameters to use when doing connection.

    Returns:
        Response object.
    """
    if mode == "get":
        return request_get(url, **kwargs)

    # should return 201.
    return request_post(url, **kwargs)

# Can return none.
# URL can't be empty.
def initialize_connection(url : str, mode : str, delay : float = 0, **kwargs):
    try:
        conn = create_connection(url, mode, **kwargs)
        
        # give it a chance to avoid bans.
        if delay > 0:
            sleep(delay)

        if conn.status_code in success_codes:
            return conn
        
        print(f"The url: {url} returned {conn.status_code}")
    except exceptions.Timeout:
        print(f"The connection timed out for {url}")
    except exceptions.TooManyRedirects:
        print(f"The {url} has many redirections.")
    except exceptions.RequestException as err:
        print(f"Can't connect to {url}, because: {err}")

    return None