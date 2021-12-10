from time import sleep
from selenium import webdriver
from selenium.webdriver.firefox.options import Options
from config import CONFIG

# Screenshot a given site and save it to the given path
# def screenshot_website(site, filename):
#     options = Options() # Create a new options object
#     options.headless = True # Avoid the windows popping up
    
#     driver = webdriver.Firefox(options=options) # Instanciate a new Firefox driver
#     driver.implicitly_wait(10) # Wait for a maximum of 10 seconds for the page to load
#     driver.get(site) # Load the host
    
#     sleep(CONFIG["screenshot_delay"]) # Wait for the page to fully load
    
#     driver.get_screenshot_as_file(filename) # Screenshot the page
#     driver.quit() # Quit the driver