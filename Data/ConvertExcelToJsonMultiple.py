import pandas
import json
import os
import sys

currentDir = os.getcwd()
fileName = ["DataDemo.xlsx", "DialogTutorial.xlsx"]
sheetName = [["Worksheet"],
             ["Tutorial", "Worksheet2"]]

excelData = pandas.DataFrame(columns = ["Id", "Character", "SpriteName", "SpritePosition", "Dialog", "Desc"])

for i in range(len(fileName)):
    excelPath = os.path.join(currentDir, fileName[i])
    
    for j in range(len(sheetName[i])):
        # Read data from excel file
        excelData = excelData.concat(pandas.read_excel(excelPath, sheet_name=sheetName[i][j]), ignore_index = True)

# Change excel data to Json and Write to file
outputName = "FullDialog.json"
jsonPath = os.path.join(currentDir, outputName)
jsonData = excelData.to_json(jsonPath, orient='records', force_ascii = False)

print("Changed excel to json file successfully.")
