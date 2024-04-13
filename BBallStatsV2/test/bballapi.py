from euroleague_api import play_by_play_data
import pandas as pd 
print("aa")

season = 2023
game_code = 239

df = play_by_play_data.get_game_play_by_play_data(season, game_code)

df.to_csv("playbyplay.csv",  encoding='utf-8')
