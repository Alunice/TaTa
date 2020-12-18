#include "SDL/sdl.h"
#include <math.h>

#define WIDTH  256
#define HEIGHT 256

struct Point
{
	int dx, dy;

	int DistSq() const { return dx*dx + dy*dy; }
};

struct Grid
{
	Point grid[HEIGHT][WIDTH];
};

Point inside = { 0, 0 };
Point empty = { 9999, 9999 };
Grid grid1, grid2;

Point Get( Grid &g, int x, int y )
{
	// OPTIMIZATION: you can skip the edge check code if you make your grid 
	// have a 1-pixel gutter.
	if ( x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT )
		return g.grid[y][x];
	else
		return empty;
}

void Put( Grid &g, int x, int y, const Point &p )
{
	g.grid[y][x] = p;
}

void Compare( Grid &g, Point &p, int x, int y, int offsetx, int offsety )
{
	Point other = Get( g, x+offsetx, y+offsety );
	other.dx += offsetx;
	other.dy += offsety;

	if (other.DistSq() < p.DistSq())
		p = other;
}

void GenerateSDF( Grid &g )
{
	// Pass 0
	for (int y=0;y<HEIGHT;y++)
	{
		for (int x=0;x<WIDTH;x++)
		{
			Point p = Get( g, x, y );
			Compare( g, p, x, y, -1,  0 );
			Compare( g, p, x, y,  0, -1 );
			Compare( g, p, x, y, -1, -1 );
			Compare( g, p, x, y,  1, -1 );
			Put( g, x, y, p );
		}

		for (int x=WIDTH-1;x>=0;x--)
		{
			Point p = Get( g, x, y );
			Compare( g, p, x, y, 1, 0 );
			Put( g, x, y, p );
		}
	}

	// Pass 1
	for (int y=HEIGHT-1;y>=0;y--)
	{
		for (int x=WIDTH-1;x>=0;x--)
		{
			Point p = Get( g, x, y );
			Compare( g, p, x, y,  1,  0 );
			Compare( g, p, x, y,  0,  1 );
			Compare( g, p, x, y, -1,  1 );
			Compare( g, p, x, y,  1,  1 );
			Put( g, x, y, p );
		}

		for (int x=0;x<WIDTH;x++)
		{
			Point p = Get( g, x, y );
			Compare( g, p, x, y, -1, 0 );
			Put( g, x, y, p );
		}
	}
}

int main( int argc, char* args[] )
{
    if ( SDL_Init( SDL_INIT_VIDEO ) == -1 )
        return 1;

    SDL_Surface *screen = SDL_SetVideoMode( WIDTH, HEIGHT, 32, SDL_SWSURFACE );
    if ( !screen )
        return 1;

	// Initialize the grid from the BMP file.
    SDL_Surface *temp = SDL_LoadBMP( "test.bmp" );
	temp = SDL_ConvertSurface( temp, screen->format, SDL_SWSURFACE ); 
	SDL_LockSurface( temp );
	for( int y=0;y<HEIGHT;y++ )
	{
		for ( int x=0;x<WIDTH;x++ )
		{
			Uint8 r,g,b;
			Uint32 *src = ( (Uint32 *)( (Uint8 *)temp->pixels + y*temp->pitch ) ) + x;
			SDL_GetRGB( *src, temp->format, &r, &g, &b );
			
			// Points inside get marked with a dx/dy of zero.
			// Points outside get marked with an infinitely large distance.
			if ( g < 128 )
			{
				Put( grid1, x, y, inside );
				Put( grid2, x, y, empty );
			} else {
				Put( grid2, x, y, inside );
				Put( grid1, x, y, empty );
			}
		}
	}
	SDL_UnlockSurface( temp );

	// Generate the SDF.
	GenerateSDF( grid1 );
	GenerateSDF( grid2 );
	
	// Render out the results.
	SDL_LockSurface( screen );
	for( int y=0;y<HEIGHT;y++ )
	{
		for ( int x=0;x<WIDTH;x++ )
		{
			// Calculate the actual distance from the dx/dy
			int dist1 = (int)( sqrt( (double)Get( grid1, x, y ).DistSq() ) );
			int dist2 = (int)( sqrt( (double)Get( grid2, x, y ).DistSq() ) );
			int dist = dist1 - dist2;

			// Clamp and scale it, just for display purposes.
			int c = dist*3 + 128;
			if ( c < 0 ) c = 0;
			if ( c > 255 ) c = 255;

			Uint32 *dest = ( (Uint32 *)( (Uint8 *)screen->pixels + y*screen->pitch ) ) + x;
			*dest = SDL_MapRGB( screen->format, c, c, c );
		}
	}
	SDL_UnlockSurface( screen );
	SDL_Flip( screen );

	// Wait for a keypress
	SDL_Event event;
	while( true )
	{
		if ( SDL_PollEvent( &event ) ) 
		switch( event.type )
		{
		case SDL_QUIT:
		case SDL_KEYDOWN:
			return true;
		}
	}

	return 0;
}
