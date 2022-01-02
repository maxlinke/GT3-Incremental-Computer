queue is just for work commands
three screens
    - main
    - store
    - help
beginning message
    - Welcome! Type "run" to start the program, type "help" to see a list of other commands.
commands
    - run
    - halt
    - help
    - save
    - load
    - store
        - buy (how)
        - sell (how)

cores auto position their things
first processors, then schedulers, then coolers
buy processor 1 -> buys a processor and places it on core 1
upgrade and sell and move do need ids however (processor # + "slot")

buy/sell options (always basic)
- core (provides slots for modules) (CAN'T SELL)
- processor (takes tasks and produces Cr)
- scheduler (creates tasks)
- cooler (reduces temperature of a core)

processors have upgrades and levels
you buy a processor at a level (each level is bigger, levels 1, 2, 3)
and you can upgrade each one _ * ** ***

i'll have to figure out how to do temperature. while the processors give a linear amount of "energy", the temperature's increase should slow down over time as it approaches the temp of the processors, physically speaking
also the cores will have to throttle if they get too hot


