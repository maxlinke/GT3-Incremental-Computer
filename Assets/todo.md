core
    unlock (each core costs more)
    upgrade same cost for each core, but increasing costs
    should all be fairly expensive, as it's kind of a global upgrade

core components
    you buy a level. each level has its own size
    each level can be upgraded. the size doesn't change but certain properties do
    upgrades should have diminishing returns (1x, 2x, 3x, 4x base speed is effectively 2x, 1.5x, 1.3333x if you look at the relative upgrades)
    can i make a data structure for this stuff? preferably something generic so i have less code to write?
    the serialized part will just be ints. int level, int upgrades. everything can be gotten via those ints. processorLevel[level].upgrade[upgrade].speed for example
    i mean i could use consts and static readonlys. ProcessorLevel.GetLevel and such. wouldn't be that bad. less inheritance because static but a) i don't need object references and b) if it's similar enough i can still have the store be generic pieces. 
    AND the levels themselves can be objects, just gotten via static means, but after that abstraction etc is fair game again!

store
    obviously generate the pages
    update the entries when the currency changes
    component adding and removing should be simple enough? 
    also yeah, the whole buy, sell and move commands

savedata
    do the auto-save on quit again, with the prompt on reopen and a little gain for the time spent closed (if running)

temperature
    the whole impulse thing. basically just a lerp i guess. if the temp is 21 and the impulse has 100 as the target and a lerp of 0.1, it's going to add a bigger number than if the temp is already 90 and the impulse is identical.

make the game canvas constant relative size thing and parent everything inside to an object forced to 640 by 480 (and disable scaling, like, just set the resolution to 640x480 if it isn't)

