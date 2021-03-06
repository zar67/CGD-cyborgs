from flask import Flask

app = Flask(__name__)

#key = players name
#value = score
highscores = dict()
highscores["liam"] = "5000"
highscores["zoe"] = "-1000"
highscores["ed"] = "-325"
highscores["matt"] = "45"
highscores["stan"] = "0"

@app.route("/")
def hello_world():
    return "<p>Hello, World!</p>"

@app.route("/SetScore/<name>/<score>")
def setScore(name :str, score : str):
    reply = "score is not int"
    if(score is int):
        highscores[name] = score
        reply = f"Score Added {name} : {score}"
    return reply

@app.route("/GetScore/<name>")
def getScore(name :str):
    return highscores[name] 

@app.route("/IncrementScore/<name>/<score>")
def incrementScore(name :str, score : str):
    if name in highscores:
        highscores[name] = str(int(highscores[name]) +  int(score))
    else:
        highscores[name] = score
    
    reply = f"Score Incremented {name} : {score}"
    return reply

@app.route("/DecrementScore/<name>/<score>")
def decrementScore(name :str, score : str):
    if name in highscores:
        highscores[name] = str(int(highscores[name]) -  int(score))
    else:
        highscores[name] =  str(-int(score))
    
    reply = f"Score Decremented {name} : {score}"
    return reply

@app.route("/GetAllScores")
def getAllScores():
    return str(highscores)
