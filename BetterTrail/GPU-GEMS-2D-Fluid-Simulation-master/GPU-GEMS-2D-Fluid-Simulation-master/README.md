# 2D-Fluid-Simulation

This a port of the GPU GEMS article on fluid simulation to Unity. Well kinda. Its is mostly based on a 2D fluid sim found on the [Little Grasshopper blog](http://prideout.net/blog/). That project was based on the GPU GEMS article also. I found he had simplified the code a little and his way of doing things was much easier to read.

There are a lot of stages to perform per frame and getting these to all work in the right order was a little bit of a pain. I was quite surprised by how many graphics blit operations need to be performed each frame. There needs to be about 60 per frame. This is a huge amount of work for the GPU to do but the frame rate is still about 60 fps which is not bad. You can decrease the number of jacobi iterations which take up the majority of the work that needs to be done. The quality of the fluid simulation will suffer if it is decreased too much as they are needed to compute a divergent free fluid simulation.

You can download a Unity package [here](https://app.box.com/s/yxdudtarxfo5jnwj4u2sbijwvs8lof55).


![2D Fluid Sim](https://static.wixstatic.com/media/1e04d5_7a1d7500c008475fa6546bb70f5db569~mv2.jpg/v1/fill/w_486,h_486,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_7a1d7500c008475fa6546bb70f5db569~mv2.jpg)
