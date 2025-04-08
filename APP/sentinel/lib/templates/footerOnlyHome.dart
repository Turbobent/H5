import 'package:sentinel/main.dart';
import 'package:flutter/material.dart';

class FooterOnlyHome extends StatelessWidget {
  const FooterOnlyHome({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      color: const Color.fromARGB(255, 243, 237, 247),
      height: 56,
      child: Center(
        child: IconButton(
          icon: const Icon(Icons.cottage),
          onPressed: () {
            Navigator.push(
              context,
              MaterialPageRoute(builder: (context) => const HomeScreen()),
            );
          },
        ),
      ),
    );
  }
}
